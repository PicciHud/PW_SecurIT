/*
 * File:   main.c
 * Author: SecurIT - Gruppo 7 
 * Alessandro Bonaldo, Davide Piccinato, Saracco Filippo
 *
 * Created on 3 maggio 2023, 15.37
 */

// PIC16F877A Configuration Bit Settings

// 'C' source line config statements

// CONFIG
#pragma config FOSC = HS        // Oscillator Selection bits (RC oscillator)
#pragma config WDTE = OFF       // Watchdog Timer Enable bit (WDT enabled)
#pragma config PWRTE = ON       // Power-up Timer Enable bit (PWRT disabled)
#pragma config BOREN = ON       // Brown-out Reset Enable bit (BOR enabled)
#pragma config LVP = ON         // Low-Voltage (Single-Supply) In-Circuit Serial Programming Enable bit (RB3/PGM pin has PGM function; low-voltage programming enabled)
#pragma config CPD = OFF        // Data EEPROM Memory Code Protection bit (Data EEPROM code protection off)
#pragma config WRT = OFF        // Flash Program Memory Write Enable bits (Write protection off; all program memory may be written to by EECON control)
#pragma config CP = OFF         // Flash Program Memory Code Protection bit (Code protection off)

// #pragma config statements should precede project file includes.
// Use project enums instead of #define for ON and OFF.

#include <xc.h>
#include <stdlib.h>             // for srand and rand use
#include <time.h>               // for time functions (to use with srand and rand functions)
#include <stdio.h>

// preprocessor directives
#define _XTAL_FREQ 20000000     // oscillator frequency

#define LCD_EN PORTEbits.RE1
#define LCD_RS PORTEbits.RE2
#define LCDPORT PORTD

#define LCDPORT_DIR TRISD
#define LCDPORT_EN_DIR TRISEbits.TRISE1
#define LCDPORT_RS_DIR TRISEbits.TRISE2

#define L_ON    0x0F
#define L_OFF   0x08
#define L_CLR   0x01
#define L_L1    0x80
#define L_L2    0xC0
#define L_NCR   0x0C
#define L_CFG   0x38

#define COMMAND 0
#define DATA 1

/* ---------------  GLOBAL VARIABLES ------------------- */

// keypad variables
unsigned char keypressed = 0;                                                           // numerical weight of key pressed
char keyok;                                                                             // variable that shows if any key of keypad has been pressed

const unsigned char colMask[3] = {
    //76543210 bit position
    0b11111110, // Column 1 => RB0
    0b11111101, // Column 2 => RB1
    0b11111011  // Column 3 => RB2
};

// filter data read from PORTD
const unsigned char rowMask[4] = {
    0b00000001, // Riga 1
    0b00000010, // Riga 2
    0b00000100, // Riga 3
    0b00001000  // Riga 4
};

// variables that contain the row and col where we are
unsigned char colScan = 0;
unsigned char rowScan = 0;

// Array col valore del tasto presente sulla scheda
// ATTENZIONE: l'ordine corretto è dal basso verso l'alto, altrimenti
//             sono invertiti i valori associati ai tasti lungo la colonna
const unsigned char keys[] = {'*', 7, 4, 1, 0, 8, 5, 2, '#', 9, 6, 3};                  // Key sequence (0 -> '*', 1 -> '7', 2 -> '4', ...)


/* -------------- FUNCTION PROTOTYPES ----------------- */

// LCD
void lcd_init(void);
void lcd_cmd(unsigned char val); 
void lcd_dat(unsigned char val);
void lcd_str(const char*);
void lcd_send(char, char);                                                              // Funzione che carica il dato char, ossia l'informazione che riceve, in una delle due memorie

// KeyPad
void initKeyPad(void);

// Code generator
char* random_string(void);

/* ------------------- MAIN PROGRAM -------------------- */
int main()
{
    // registers initalization
    TRISB = 0x00;                                                                       // all PORTB ports as OUTPUT
    INTCON |= 0xA0;                                                                     // interrupt management
    OPTION_REG = 0x05;                                                                  // prescaler configuration
    TMR0 = 6;                                                                           // timer0 preload
    
    // initialization
    lcd_init();
    initKeyPad();                                                                       // bisogna posizionarlo dopo l'lcd_str altrimenti non funziona
    
    lcd_send(L_CLR, COMMAND);
    lcd_str("waiting...");

    // code loop
    while(1)
    {
        TRISB = 0x00;                                                                   // set RB7 as output
        TRISD = 0x0F;                                                                   // initialize 4 LSB of PORTD
        
        // if "#" button has been pressed
        //------------- keypad ---------------
        for (colScan = 0; colScan < 3; colScan++)
        {
            // Qui metto il codice che controlla PORTD
            PORTB = PORTB | 0x07;                                                       // 111 all columns to 1
            PORTB = PORTB & colMask[colScan];                                           // set current column to zero to select it

            for (rowScan = 0; rowScan < 4; rowScan++)                                   // scan one row at a time and
            {
                if (!(PORTD & rowMask[rowScan]))                                        // if "0", button has been pressed
                {
                    __delay_ms(5);

                    if (!(PORTD & rowMask[rowScan]))
                    {
                        // evaluate which button has been pressed
                        keypressed = rowScan + (4 * colScan);                           // expression to calculate which one of the button has been pressed
                        keyok = 1;                                                      // button has been pressed (logical state)
                    }
                }
            }

            if (keyok)                                                                  // if any key of keypad has been pressed
            {
                // GESTIRE LA PRESSIONE DEL PULSANTE: se l'utente tiene premuto un pulsante della keyboard in ogni caso deve essere rilevato solo una pressione
                if(keypressed == 8)                                                     // if '#' key has been pressed
                {
                    // generate random code (5 numbers)
                    lcd_send(L_CLR, COMMAND);                                           // clear display
                    //lcd_str("12345");
                    char* code = random_string();                                       // generate a new random code
                    lcd_str(code);
                    
                    // send code to the gateway (Raspberry Pi) if master select this PIC
                }
                
                // test for debug
                if (keypressed == 7)
                {
                    lcd_send(L_CLR, COMMAND);
                    lcd_str("28753");
                }
                
                keyok = 0;                                                              // reset variable for next clicks

                // keep in a loop cicle until key is not released
                PORTD = PORTD | 0x0F;
                while ((PORTD & 0x0F) != 0x0F)
                {
                    PORTD = PORTD | 0x0F;
                    continue;
                }
                    
                //__delay_ms(200);                                                      // DO NOT USE: delay that seems to establish keypad click
            }
        }
    }
    
    return 0;
}


/* ------------------  FUNCTION BODY ------------------- */


// --------------- LCD functions ------------------- //
void lcd_dat(unsigned char val)
{
    LCD_EN = 1;
    
    LCDPORT = val;
    LCD_RS = 1;
    __delay_ms(3);
    LCD_EN = 0;
    __delay_ms(3);
    
    LCD_EN = 1;
}

void lcd_cmd(unsigned char val)
{
    LCD_EN = 1;
    
    LCDPORT = val;
    LCD_RS = 0;
    __delay_ms(3);
    LCD_EN = 0;                                                                         // Falling edge for data loading
    __delay_ms(3);
    
    LCD_EN = 1;
}

void lcd_init(void)
{
    TRISD = 0x00;
    TRISE = 0x00;
    LCD_EN = 0;
    LCD_RS = 0;
    __delay_ms(20);
    LCD_EN = 1;
    
    lcd_cmd(L_CFG);
    __delay_ms(5);
    lcd_cmd(L_CFG);
    __delay_ms(1);
    lcd_cmd(L_CFG);                                                             
    lcd_cmd(L_OFF);
    lcd_cmd(L_ON);
    lcd_cmd(L_CLR);
    lcd_cmd(0b00001100);
    lcd_cmd(L_L1);
}

void lcd_str(const char* str)
{
    unsigned char i = 0;
    
    while (str[i] != 0)
    {
        lcd_dat(str[i]);
        i++;
    }
}


void lcd_send(char dato, char tipo)                                                     // 1st param: data - 2nd param: data type (if 1 data to write on display, if 0 command/setting)
{             
    
    // Configurazione porte di connessione
    LCDPORT_DIR = 0x00;                                                                 // TRISD 0000 0000 - OUTPUT
    LCDPORT_EN_DIR = 0;
    LCDPORT_RS_DIR = 0;
    // Procedura di scrittura sul display
    LCD_EN = 1;
    LCDPORT = dato;                                                                     // save data into this variable
    LCD_RS = tipo;                                                                      
    __delay_ms(3);  
    LCD_EN = 0;
    __delay_ms(3);
    LCD_EN = 1;
}


// --------------- keypad functions ------------------- //
void initKeyPad() {
  //set input/output pins
    TRISD |= 0x0f;                                                                      // set only 4 MSB leds of PORTD as output
    TRISB &= ~0x07;                                                                     // input keys
}


// --------------- code generator function ------------ //
char* random_string(void) {
    static char str[6];
    const char charset[] = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    //srand(time(NULL));                                                                // initialize random generator. WARNING: the compiler rise an error if this
                                                                                        // code uncommented, probably due to "time" function call in srand.

    for (int i = 0; i < 5; i++) {
        int index = rand() % (sizeof(charset) - 1);                                     // select a random charachter from the charset array
        str[i] = charset[index];                                                        // for 5 times add the charachter into code array
    }

    str[5] = '\0';                                                                      // add string terminator

    return str;                                                                         // return the code array as pointer
}